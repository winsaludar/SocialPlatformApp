import { NextResponse } from "next/server";

const UNSAFE_CURRENT_USER_COOKIE = "currentUser";
const SAFE_CURRENT_USER_COOKIE = "currentUserHttpOnly";

export function middleware(request) {
  const currentUser = request.cookies.get(UNSAFE_CURRENT_USER_COOKIE)?.value;
  const currentUserHttpOnly = request.cookies.get(
    SAFE_CURRENT_USER_COOKIE
  )?.value;
  const protectedRoutes = process.env.PROTECTED_ROUTES;
  const isRouteProtected = protectedRoutes.includes(request.nextUrl.pathname);

  // Convert currentUser cookie to httpOnly
  if (currentUser) {
    const cookieValue = JSON.parse(currentUser);
    const response = NextResponse.next();
    response.cookies.set({
      name: SAFE_CURRENT_USER_COOKIE,
      value: JSON.stringify(cookieValue),
      httpOnly: true,
    });

    request.cookies.delete(UNSAFE_CURRENT_USER_COOKIE);
    response.cookies.delete(UNSAFE_CURRENT_USER_COOKIE);

    return response;
  }

  // Check if user is authenticated
  if (isRouteProtected && !currentUserHttpOnly) {
    return NextResponse.redirect(new URL("/login", request.url));
  }

  // Check if authenticated user's token is expired
  if (
    isRouteProtected &&
    currentUserHttpOnly &&
    Date.now() > Date.parse(JSON.parse(currentUserHttpOnly).expiresAt)
  ) {
    const response = NextResponse.redirect(new URL("/login", request.url));
    response.cookies.delete(SAFE_CURRENT_USER_COOKIE);

    return response;
  }

  // Prevent authenticated user from accessing login and register page
  if (
    currentUserHttpOnly &&
    Date.now() < Date.parse(JSON.parse(currentUserHttpOnly).expiresAt) &&
    (request.nextUrl.pathname === "/login" ||
      request.nextUrl.pathname === "/register")
  ) {
    return NextResponse.redirect(new URL("/", request.url));
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
