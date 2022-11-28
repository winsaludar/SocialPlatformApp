import Head from "next/head";

export default function Layout({ children }) {
  return (
    <>
      <Head>
        <title>
          {`${children.props.title} - ${process.env.NEXT_PUBLIC_APP_NAME} by Erwin Saludar (https://github.com/winsaludar)`}
        </title>
        <meta name="viewport" content="width=device-width, initial-scale=1.0" />
      </Head>

      <div className="container">{children}</div>
    </>
  );
}
