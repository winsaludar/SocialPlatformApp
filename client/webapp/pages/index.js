import Head from "next/head";
import Login from "../components/authentication/Login";
import AppList from "../components/app/AppList";

const appName = process.env.NEXT_PUBLIC_APP_NAME;
const isLoggedIn = true; // TODO: USE REAL LOGIN SESSION

export async function getStaticProps() {
  const title = isLoggedIn ? "Homepage" : "Login";
  return { props: { title: `${title} | ${appName}` } };
}

function getLoginLayout() {
  return (
    <>
      <Login />
    </>
  );
}

function getLandingLayout() {
  return (
    <>
      <AppList />
    </>
  );
}

export default function Home() {
  return isLoggedIn ? getLandingLayout() : getLoginLayout();
}
