// middleware.ts
import { NextResponse } from "next/server";
import type { NextRequest } from "next/server";
import { serverTokenStorage } from "./lib/server/token-storage";
import { APP_ROUTES } from "./app-routes";

export async function middleware(request: NextRequest) {
  const { pathname, searchParams } = request.nextUrl;

  // Skip public routes
  if (!pathname.startsWith("/app")) return NextResponse.next();

  // Check auth
  const refreshToken = await serverTokenStorage.hasRefreshToken();
  if (!refreshToken) {
    return NextResponse.redirect(new URL(APP_ROUTES.AUTH.SIGN_IN, request.url));
  }

  // Check kitchen in query string
  // const kitchen = searchParams.get("kitchen");
  // if (!kitchen && !pathname.startsWith(APP_ROUTES.APP.KITCHEN_SELECTION)) {
  //   // Check if user has a default kitchen in cookie
  //   const selectedKitchen = request.cookies.get("selectedKitchen");

  //   if (selectedKitchen) {
  //     // Redirect with kitchen in query string
  //     const url = new URL(request.url);
  //     url.searchParams.set("kitchen", selectedKitchen.value);
  //     return NextResponse.redirect(url);
  //   }

  //   // No kitchen at all - redirect to selection
  //   const selectUrl = new URL(APP_ROUTES.APP.KITCHEN_SELECTION, request.url);
  //   selectUrl.searchParams.set("redirect", pathname);
  //   return NextResponse.redirect(selectUrl);
  // }

  return NextResponse.next();
}

export const config = {
  matcher: ["/app/:path*"], // only app routes
};
