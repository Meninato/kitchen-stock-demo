import axios, {
  AxiosError,
  AxiosInstance,
  AxiosRequestConfig,
  AxiosResponse,
} from "axios";

export type NoContent = null;

export interface ApiResponse<T = unknown> {
  data: T;
  message?: string;
}

export interface ApiErrorResponse<T = Record<string, string>> {
  message: string;
  metadata: T;
}

export class ApiError<T = Record<string, string>> extends Error {
  public statusCode: number;
  public metadata: T;
  public response?: AxiosResponse<ApiErrorResponse<T>>;

  constructor(
    message: string,
    statusCode: number,
    metadata: T,
    response?: AxiosResponse<ApiErrorResponse<T>>
  ) {
    super(message);
    this.name = "ApiError";
    this.statusCode = statusCode;
    this.metadata = metadata;
    this.response = response;
    Object.setPrototypeOf(this, ApiError.prototype);
  }
}

class ApiClient {
  private client: AxiosInstance;
  private isRefreshing = false;
  private failedQueue: Array<{
    resolve: (result: unknown) => void;
    reject: (error: unknown) => void;
  }> = [];

  constructor() {
    this.client = axios.create({
      baseURL: process.env.NEXT_PUBLIC_API_URL,
      timeout: parseInt(process.env.NEXT_PUBLIC_API_TIMEOUT || "30000"),
      headers: {
        "Content-Type": "application/json",
      },
      withCredentials: true,
    });

    this.setupInterceptors();
  }

  private setupInterceptors(): void {
    // Request interceptor
    this.client.interceptors.request.use(
      async (config) => {
        // Add correlation ID for distributed tracing
        config.headers["X-Correlation-Id"] = this.generateCorrelationId();

        // Log request in development
        if (process.env.NEXT_PUBLIC_ENV === "development") {
          console.log(
            `[API Request] ${config.method?.toUpperCase()} ${config.url}`,
            config.data
          );
        }

        return config;
      },
      (error) => {
        return Promise.reject(error);
      }
    );

    // Response interceptor
    this.client.interceptors.response.use(
      (response) => {
        // Log response in development
        if (process.env.NEXT_PUBLIC_ENV === "development") {
          console.log(`[API Response] ${response.config.url}`, response.data);
        }
        return response;
      },
      async (error: AxiosError<ApiErrorResponse>) => {
        const originalRequest = error.config as AxiosRequestConfig & {
          _retry?: boolean;
        };

        const isAuthRoute = originalRequest?.url?.includes("/auth");

        //Handle 401 Unauthorized
        if (
          error.response?.status === 401 &&
          !originalRequest._retry &&
          !isAuthRoute
        ) {
          if (this.isRefreshing) {
            return new Promise((resolve, reject) => {
              this.failedQueue.push({ resolve, reject });
            }).then(() => {
              return this.client(originalRequest);
            });
          }

          originalRequest._retry = true;
          this.isRefreshing = true;

          try {
            await this.refreshTokens();
            this.processQueue(null);

            return this.client(originalRequest);
          } catch (refreshError) {
            this.processQueue(refreshError);

            if (typeof window !== "undefined") {
              window.location.href = "/auth/sign-in";
            }

            throw refreshError;
          } finally {
            this.isRefreshing = false;
          }
        }

        // Transform error to our custom ApiError
        throw this.transformError(error);
      }
    );
  }

  private async refreshTokens(): Promise<void> {
    const response = await this.client.post("/auth/refresh");
    return response.data;
  }

  private processQueue(error: unknown, result: unknown = null): void {
    this.failedQueue.forEach((prom) => {
      if (error) {
        prom.reject(error);
      } else {
        prom.resolve(result);
      }
    });

    this.failedQueue = [];
  }

  private transformError<T = Record<string, string>>(
    error: AxiosError<ApiErrorResponse<T>>
  ): ApiError<T> {
    if (error.response) {
      const { data, status } = error.response;
      const message = data?.message || `Request failed with status ${status}`;
      const metadata = data?.metadata ?? ({} as T);

      return new ApiError<T>(message, status, metadata, error.response);
    }

    if (error.request) {
      const sslError = this.checkSslCertificateError(error);
      if (sslError) {
        return sslError;
      }

      return new ApiError<T>(
        "Network error. Please check your connection.",
        0,
        { error_code: "NETWORK_ERROR", type: "Infrastructure" } as T
      );
    }

    return new ApiError<T>(error.message || "An unexpected error occurred", 0, {
      error_code: "UNEXPECTED_ERROR",
      type: "Infrastructure",
    } as T);
  }

  private checkSslCertificateError<T = Record<string, string>>(
    error: AxiosError<ApiErrorResponse<T>>
  ): ApiError<T> | undefined {
    if (
      error.code === "DEPTH_ZERO_SELF_SIGNED_CERT" ||
      error.code === "UNABLE_TO_VERIFY_LEAF_SIGNATURE" ||
      error.code === "SELF_SIGNED_CERT_IN_CHAIN"
    ) {
      return new ApiError<T>(
        "SSL Certificate error. The API is using a self-signed certificate.",
        0,
        {
          error_code: "SSL_CERTIFICATE_ERROR",
          type: "Infrastructure",
          ssl_error: error.code,
          suggestion:
            "Accept the certificate or configure the API for development",
        } as T
      );
    }
  }

  private generateCorrelationId(): string {
    return `${Date.now()}-${Math.random().toString(36).substring(2, 9)}`;
  }

  // HTTP methods
  public async get<T>(
    url: string,
    config?: AxiosRequestConfig
  ): Promise<ApiResponse<T>> {
    const response = await this.client.get<ApiResponse<T>>(url, config);
    return response.data;
  }

  public async post<T, D = unknown>(
    url: string,
    data?: D,
    config?: AxiosRequestConfig
  ): Promise<ApiResponse<T>> {
    const response = await this.client.post<ApiResponse<T>>(url, data, config);
    return response.data;
  }

  public async put<T, D = unknown>(
    url: string,
    data?: D,
    config?: AxiosRequestConfig
  ): Promise<ApiResponse<T>> {
    const response = await this.client.put<ApiResponse<T>>(url, data, config);
    return response.data;
  }

  public async patch<T, D = unknown>(
    url: string,
    data?: D,
    config?: AxiosRequestConfig
  ): Promise<ApiResponse<T>> {
    const response = await this.client.patch<ApiResponse<T>>(url, data, config);
    return response.data;
  }

  //OVERLOADS
  public async delete(url: string, config?: AxiosRequestConfig): Promise<void>;
  public async delete<T>(
    url: string,
    config?: AxiosRequestConfig
  ): Promise<ApiResponse<T>>;

  public async delete<T = NoContent>(
    url: string,
    config?: AxiosRequestConfig
  ): Promise<void | ApiResponse<T>> {
    const response = await this.client.delete(url, config);

    if (response.status === 204) {
      return;
    }

    return response.data;
  }

  // File upload
  public async uploadFile<T>(
    url: string,
    file: File,
    onProgress?: (progress: number) => void
  ): Promise<ApiResponse<T>> {
    const formData = new FormData();
    formData.append("file", file);

    const response = await this.client.post<ApiResponse<T>>(url, formData, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
      onUploadProgress: (progressEvent) => {
        if (onProgress && progressEvent.total) {
          const progress = Math.round(
            (progressEvent.loaded * 100) / progressEvent.total
          );
          onProgress(progress);
        }
      },
    });

    return response.data;
  }
}

// Export singleton instance
export const apiClient = new ApiClient();
