import { useEffect, useState } from "react";

import ExploreHeader from "../src/components/chat/ExploreHeader";
import ExploreList from "../src/components/chat/ExploreList";
import MainNav from "../src/components/chat/MainNav";
import Sidebar from "../src/components/chat/Sidebar";
import styles from "../styles/ChatComponent.module.css";

export async function getStaticProps() {
  return { props: { title: "Chat" } };
}

export default function ChatPage() {
  const [servers, setServers] = useState([]);
  const [showLoader, setShowLoader] = useState(false);

  useEffect(() => {
    async function fetchData() {
      setShowLoader(true);

      const endpoint = "api/getAllServers";
      const options = {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
        },
      };

      try {
        const response = await fetch(endpoint, options);
        const result = await response.json();
        setServers(result.data);
      } catch (err) {}

      setShowLoader(false);
    }

    fetchData();
  }, []);

  return (
    <>
      <div className={styles.container}>
        <MainNav />
        <Sidebar />

        <div className={styles.content}>
          <ExploreHeader />
          <ExploreList servers={servers} showLoader={showLoader} />
        </div>
      </div>
    </>
  );
}
