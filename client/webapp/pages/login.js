import Login from "../src/components/authentication/Login";

const appName = process.env.NEXT_PUBLIC_APP_NAME;

export async function getStaticProps() {
  return { props: { title: `Login | ${appName}` } };
}

export default function LoginPage() {
  return (
    <section>
      <Login />
    </section>
  );
}
