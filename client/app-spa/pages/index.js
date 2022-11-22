import Head from "next/head";
import Login from "../components/authentication/Login";

export default function Home() {
  return (
    <>
      <Head>
        <meta name="viewport" content="width=device-width, initial-scale=1.0" />
        <title>Home | Social Platform App</title>
        <meta name="description" content="" />
        <meta name="keywords" content="" />
        <link rel="canonical" href="" />
      </Head>

      <main>
        <h1>Hello, world!</h1>
        <Login />
      </main>

      <footer></footer>
    </>
  );
}
