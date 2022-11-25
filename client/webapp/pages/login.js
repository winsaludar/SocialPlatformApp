import Cookies from "js-cookie";
import { useRouter } from "next/router";

import Login from "../src/components/authentication/Login";

const appName = process.env.NEXT_PUBLIC_APP_NAME;

export async function getStaticProps() {
  return { props: { title: `Login | ${appName}` } };
}

export default function LoginPage() {
  const router = useRouter();

  return (
    <section>
      <Login
        registerLink="/register"
        onSubmitSuccessfulCallback={(response) => {
          Cookies.set("currentUser", JSON.stringify(response));
          router.push("/");
        }}
      />
    </section>
  );
}
