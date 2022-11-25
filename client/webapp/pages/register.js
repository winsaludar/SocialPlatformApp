import Register from "../src/components/authentication/Register";

const appName = process.env.NEXT_PUBLIC_APP_NAME;

export async function getStaticProps() {
  return { props: { title: `Register | ${appName}` } };
}

export default function RegisterPage() {
  return (
    <section>
      <Register />
    </section>
  );
}
