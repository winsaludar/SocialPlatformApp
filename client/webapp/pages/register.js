import Registration from "../components/authentication/Registration";

const appName = process.env.NEXT_PUBLIC_APP_NAME;

export async function getStaticProps() {
  return { props: { title: `Register | ${appName}` } };
}

export default function Register() {
  return (
    <section>
      <Registration />
    </section>
  );
}
