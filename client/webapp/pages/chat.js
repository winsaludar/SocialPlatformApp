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
  const [serverFilter, setServerFilter] = useState(null);

  useEffect(() => {
    let ignore = false;

    (async function () {
      setShowLoader(true);
      setServers([]);

      let endpoint = "api/getAllServers";
      if (serverFilter) endpoint += `?name=${serverFilter}`;

      const options = {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
        },
      };

      try {
        const response = await fetch(endpoint, options);
        const result = await response.json();
        if (!ignore) setServers(result.data);
      } catch {}

      setShowLoader(false);
    })();

    return () => {
      ignore = true;
    };
  }, [serverFilter]);

  return (
    <>
      <div className={styles.container}>
        <MainNav />
        <Sidebar />

        <div className={styles.content}>
          <ExploreHeader
            onTextChange={(e) => {
              setServerFilter(e.target.value);
            }}
          />
          <ExploreList servers={servers} showLoader={showLoader} />
        </div>
      </div>
    </>
  );
}
