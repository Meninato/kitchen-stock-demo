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
    resolve: (token: string) => void;
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
      (config) => {
        // Add auth token if available
        const token = this.getAuthToken();
        if (token) {
          config.headers.Authorization = `Bearer ${token}`;
        }

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

        // Handle 401 Unauthorized
        if (error.response?.status === 401 && !originalRequest._retry) {
          if (this.isRefreshing) {
            return new Promise((resolve, reject) => {
              this.failedQueue.push({ resolve, reject });
            }).then((token) => {
              originalRequest.headers!.Authorization = `Bearer ${token}`;
              return this.client(originalRequest);
            });
          }

          originalRequest._retry = true;
          this.isRefreshing = true;

          try {
            const newToken = await this.refreshToken();
            this.processQueue(null, newToken);
            originalRequest.headers!.Authorization = `Bearer ${newToken}`;
            return this.client(originalRequest);
          } catch (refreshError) {
            this.processQueue(refreshError, null);
            this.handleAuthError();
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

  private processQueue(error: unknown, token: string | null = null): void {
    this.failedQueue.forEach((prom) => {
      if (error) {
        prom.reject(error);
      } else {
        prom.resolve(token!);
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
      return new ApiError<T>(
        "Network error. Please check your connection.",
        0,
        { errorCode: "NETWORK_ERROR", type: "Infrastructure" } as T
      );
    }

    return new ApiError<T>(error.message || "An unexpected error occurred", 0, {
      errorCode: "UNEXPECTED_ERROR",
      type: "Infrastructure",
    } as T);
  }

  private getAuthToken(): string | null {
    // Get from localStorage or cookie
    if (typeof window !== "undefined") {
      return localStorage.getItem("access_token");
    }
    return null;
  }

  private async refreshToken(): Promise<string> {
    try {
      const response = await this.post<{
        accessToken: string;
        refreshToken: string;
      }>("/auth/refresh", {
        refreshToken: localStorage.getItem("refresh_token"),
      });

      localStorage.setItem("access_token", response.data.accessToken);
      localStorage.setItem("refresh_token", response.data.refreshToken);

      return response.data.accessToken;
    } catch (error) {
      throw error;
    }
  }

  private handleAuthError(): void {
    // Clear tokens and redirect to login
    if (typeof window !== "undefined") {
      localStorage.removeItem("access_token");
      localStorage.removeItem("refresh_token");
      window.location.href = "/login";
    }
  }

  private generateCorrelationId(): string {
    return `${Date.now()}-${Math.random().toString(36).substr(2, 9)}`;
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
