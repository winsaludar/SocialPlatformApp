import { useState } from "react";

import Register from "../src/components/authentication/Register";
import Loader from "../src/components/Loader";

const appName = process.env.NEXT_PUBLIC_APP_NAME;

export async function getStaticProps() {
  return { props: { title: `Register | ${appName}` } };
}

export default function RegisterPage() {
  const [showLoader, setShowLoader] = useState(false);

  return (
    <section>
      {showLoader && <Loader />}

      <Register
        loginLink="/login"
        onPreSubmitCallback={() => setShowLoader(true)}
        onFailCallback={() => setShowLoader(false)}
        onSuccessCallback={() => setShowLoader(false)}
      />
    </section>
  );
}
