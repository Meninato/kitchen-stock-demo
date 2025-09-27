import { AUTH_API_ROUTES } from "@/modules/auth/api/endpoint";
import axios, {
  AxiosError,
  AxiosInstance,
  AxiosRequestConfig,
  AxiosResponse,
} from "axios";

export type NoContent = null;

export interface ApiPaginationRequest {
  page: number;
  pageSize: number;
  sortBy?: string;
  sortDescending?: boolean;
}

export interface ApiPaginationResponse {
  page: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
  hasPrevious: boolean;
  hasNext: boolean;
}

export interface ApiResponse<T = unknown> {
  data: T;
  pagination?: ApiPaginationResponse;
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

// Export helpers
export function isApiError(error: unknown): error is ApiError {
  return error instanceof ApiError;
}

export function getErrorStatusCode(error: unknown): number {
  if (isApiError(error)) {
    return error.statusCode;
  }

  if (error instanceof AxiosError) {
    return error.response?.status ?? 0;
  }

  if (error instanceof Error) {
    // maybe default to 500 for generic Errors
    return 500;
  }

  return 0; // Unknown error type
}

export function getErrorMessage(error: unknown): string {
  if (isApiError(error)) {
    return error.message;
  }

  if (error instanceof AxiosError) {
    return error.response?.data?.message || error.message;
  }

  if (error instanceof Error) {
    return error.message;
  }

  return "An unexpected error occurred";
}

class ApiClient {
  private client: AxiosInstance;
  private isRefreshing = false;
  private refreshPromise: Promise<void> | null = null;
  private failedQueue: Array<{
    resolve: (result: unknown) => void;
    reject: (error: unknown) => void;
    config: AxiosRequestConfig;
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
          _queued?: boolean;
        };

        // If no config, reject immediately
        if (!originalRequest) {
          throw this.transformError(error);
        }

        const authRoutes = Object.values(AUTH_API_ROUTES).filter(
          (route) => route !== AUTH_API_ROUTES.ME
        );

        const shouldSkipRefresh = authRoutes.some((route) =>
          originalRequest?.url?.includes(route)
        );

        // Handle 401 Unauthorized
        if (error.response?.status === 401 && !shouldSkipRefresh) {
          // Don't retry if we've already retried this request
          if (originalRequest._retry) {
            if (process.env.NEXT_PUBLIC_ENV === "development") {
              console.log(
                `[API Client] Request already retried, rejecting: ${originalRequest.url}`
              );
            }
            throw this.transformError(error);
          }

          originalRequest._retry = true;

          // If a refresh is in progress, wait for it
          if (this.isRefreshing && this.refreshPromise) {
            if (process.env.NEXT_PUBLIC_ENV === "development") {
              console.log(
                `[API Client] Waiting for existing refresh for: ${originalRequest.url}`
              );
            }

            try {
              // Wait for the existing refresh to complete
              await this.refreshPromise;
              // Retry the original request
              return this.client(originalRequest);
            } catch (refreshError) {
              // Refresh failed, propagate the error
              throw this.transformError(
                refreshError as AxiosError<ApiErrorResponse>
              );
            }
          }

          // No refresh in progress, start one
          if (!this.isRefreshing) {
            if (process.env.NEXT_PUBLIC_ENV === "development") {
              console.log(
                `[API Client] Starting token refresh triggered by: ${originalRequest.url}`
              );
            }

            this.isRefreshing = true;

            // Create the refresh promise that all requests will share
            this.refreshPromise = this.performTokenRefresh();

            try {
              await this.refreshPromise;

              if (process.env.NEXT_PUBLIC_ENV === "development") {
                console.log(
                  `[API Client] Refresh successful, retrying: ${originalRequest.url}`
                );
              }

              // Retry the original request
              return this.client(originalRequest);
            } catch (refreshError) {
              if (process.env.NEXT_PUBLIC_ENV === "development") {
                console.error(
                  `[API Client] Refresh failed for: ${originalRequest.url}`
                );
              }
              throw this.transformError(
                refreshError as AxiosError<ApiErrorResponse>
              );
            }
          }

          // This should rarely happen, but queue the request as a fallback
          if (process.env.NEXT_PUBLIC_ENV === "development") {
            console.log(
              `[API Client] Queueing request: ${originalRequest.url}`
            );
          }

          return new Promise((resolve, reject) => {
            this.failedQueue.push({
              resolve,
              reject,
              config: originalRequest,
            });
          });
        }

        // Not a 401 or shouldn't refresh, transform and throw the error
        throw this.transformError(error);
      }
    );
  }

  private async performTokenRefresh(): Promise<void> {
    try {
      // Perform the actual refresh
      await this.refreshTokens();

      if (process.env.NEXT_PUBLIC_ENV === "development") {
        console.log("[API Client] Token refresh successful");
      }

      // Process any queued requests
      this.processQueue(null);
    } catch (error) {
      if (process.env.NEXT_PUBLIC_ENV === "development") {
        console.error("[API Client] Token refresh failed:", error);
      }

      // Process queued requests with error
      this.processQueue(error);

      // Redirect to login on refresh failure
      this.handleAuthFailure();

      throw error;
    } finally {
      // Reset refresh state
      this.isRefreshing = false;
      this.refreshPromise = null;
    }
  }

  private async refreshTokens(): Promise<void> {
    // Create a new axios instance without interceptors to avoid loops
    const refreshClient = axios.create({
      baseURL: process.env.NEXT_PUBLIC_API_URL,
      timeout: parseInt(process.env.NEXT_PUBLIC_API_TIMEOUT || "30000"),
      headers: {
        "Content-Type": "application/json",
      },
      withCredentials: true,
    });

    const response = await refreshClient.post("/auth/refresh");
    return response.data;
  }

  private processQueue(error: unknown): void {
    if (process.env.NEXT_PUBLIC_ENV === "development") {
      console.log(
        `[API Client] Processing ${this.failedQueue.length} queued requests`
      );
    }

    this.failedQueue.forEach((promise) => {
      if (error) {
        promise.reject(error);
      } else {
        // Retry the request
        this.client(promise.config).then(promise.resolve).catch(promise.reject);
      }
    });

    this.failedQueue = [];
  }

  private handleAuthFailure(): void {
    if (typeof window !== "undefined") {
      // Redirect to login page
      window.location.href = "/auth/sign-in";
    }
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
