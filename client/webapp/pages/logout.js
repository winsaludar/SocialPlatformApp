import Cookies from "js-cookie";
import { useRouter } from "next/router";
import { useEffect } from "react";

// NOTE: TEMPORARY LOGOUT ONLY
export default function LogoutPage() {
  const router = useRouter();

  useEffect(() => {
    Cookies.remove("currentUser");
    router.push("/login");
  }, [router]);
}
