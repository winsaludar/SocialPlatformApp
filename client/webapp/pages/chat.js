import { useEffect, useState } from "react";

import ExploreHeader from "../src/components/chat/ExploreHeader";
import ExploreList from "../src/components/chat/ExploreList";
import MainNav from "../src/components/chat/MainNav";
import Sidebar from "../src/components/chat/Sidebar";
import styles from "../styles/ChatComponent.module.css";

export async function getServerSideProps(context) {
  let userServers = [];

  // Make sure user cookie (where our auth token reside) exist
  let token;
  try {
    const authCookie = context.req.cookies["currentUserHttpOnly"] ?? null;
    if (!authCookie) throw "Unauthorized user";

    const parsedCookie = JSON.parse(authCookie);
    if (!parsedCookie.value) throw "Unauthorized user";

    token = parsedCookie.value;
  } catch {}

  // Fetch user servers from remote api
  try {
    const api = `${process.env.CHAT_BASE_URL}/${process.env.CHAT_GET_USER_SERVERS_ENDPOINT}`;
    const options = {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
    };

    const response = await fetch(api, options);
    userServers = await response.json();
  } catch {}

  return {
    props: {
      title: "Chat",
      userServers: userServers,
    },
  };
}

export default function ChatPage({ userServers }) {
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
        <MainNav userServers={userServers} />
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
