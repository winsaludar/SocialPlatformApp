import Head from "next/head";

export default function Layout({ children }) {
  return (
    <>
      <Head>
        <title>{children.props.title}</title>
      </Head>

      <main>{children}</main>
    </>
  );
}
