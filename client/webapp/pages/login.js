import Cookies from "js-cookie";
import { useRouter } from "next/router";
import { useState } from "react";

import Login from "../src/components/auth/Login";
import Loader from "../src/components/Loader";

const appName = process.env.NEXT_PUBLIC_APP_NAME;

export async function getStaticProps() {
  return { props: { title: `Login | ${appName}` } };
}

export default function LoginPage() {
  const router = useRouter();
  const [showLoader, setShowLoader] = useState(false);

  const preSubmitCallback = () => {
    setShowLoader(true);
  };

  const failCallback = () => {
    setShowLoader(false);
  };

  const successCallback = (response) => {
    Cookies.set("currentUser", JSON.stringify(response));
    router.push("/");
  };

  return (
    <section>
      {showLoader && <Loader />}

      <Login
        registerLink="/register"
        onPreSubmitCallback={preSubmitCallback}
        onFailCallback={failCallback}
        onSuccessCallback={successCallback}
      />
    </section>
  );
}
