import { NextResponse } from "next/server";

export function middleware(request) {
  const currentUser = request.cookies.get("currentUser")?.value;
  const protectedRoutes = process.env.PROTECTED_ROUTES;
  const isRouteProtected = protectedRoutes.includes(request.nextUrl.pathname);

  // Check if user is authenticated
  if (isRouteProtected && !currentUser) {
    return NextResponse.redirect(new URL("/login", request.url));
  }

  // Check if authenticated user's token is expired
  if (
    isRouteProtected &&
    currentUser &&
    Date.now() > Date.parse(JSON.parse(currentUser).expiresAt)
  ) {
    return NextResponse.redirect(new URL("/login", request.url));
  }
}

export const config = {
  matcher: [
    /*
     * Match all request paths except for the ones starting with:
     * - api (API routes)
     * - _next
     * - favicon.ico (favicon file)
     */
    "/((?!api|_next|favicon.ico).*)",
  ],
};
