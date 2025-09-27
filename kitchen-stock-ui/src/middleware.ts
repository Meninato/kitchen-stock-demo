// middleware.ts
import { NextResponse } from "next/server";
import type { NextRequest } from "next/server";
import { serverTokenStorage } from "./lib/server/token-storage";
import { APP_ROUTES } from "./app-routes";

export async function middleware(request: NextRequest) {
  const { pathname } = request.nextUrl;

  // Skip public routes
  if (!pathname.startsWith("/app")) return NextResponse.next();

  // Check auth
  const refreshToken = await serverTokenStorage.hasRefreshToken();
  if (!refreshToken) {
    return NextResponse.redirect(new URL(APP_ROUTES.AUTH.SIGN_IN, request.url));
  }

  return NextResponse.next();
}

export const config = {
  matcher: ["/app/:path*"], // only app routes
};
