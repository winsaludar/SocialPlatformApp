import { useState } from "react";

import Register from "../src/components/auth/Register";
import Loader from "../src/components/Loader";

export async function getStaticProps() {
  return { props: { title: "Register" } };
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
