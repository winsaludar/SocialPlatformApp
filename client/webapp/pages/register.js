import Registration from "../components/authentication/Registration";

const appName = process.env.NEXT_PUBLIC_APP_NAME;

export default function Register() {
  return <Registration title={`Register | ${appName}`} />;
}
