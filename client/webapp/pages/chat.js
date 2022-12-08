import { useEffect, useState } from "react";

import ExploreHeader from "../src/components/chat/ExploreHeader";
import ExploreList from "../src/components/chat/ExploreList";
import ExploreSidebar from "../src/components/chat/ExploreSidebar";
import MainNav from "../src/components/chat/MainNav";
import ServerSidebar from "../src/components/chat/ServerSidebar";
import styles from "../styles/ChatComponent.module.css";

export async function getServerSideProps(context) {
  // Make sure user cookie (where our auth token reside) exist
  let token;
  try {
    const authCookie = context.req.cookies["currentUserHttpOnly"] ?? null;
    if (!authCookie) throw "Unauthorized user";

    const parsedCookie = JSON.parse(authCookie);
    if (!parsedCookie.value) throw "Unauthorized user";

    token = parsedCookie.value;
  } catch {}

  const httpGetOptions = {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
  };

  // Fetch user info from remote api
  let userInfo = {};
  try {
    const api = `${process.env.CHAT_BASE_URL}/${process.env.CHAT_GET_USER_INFO_ENDPOINT}`;
    const response = await fetch(api, httpGetOptions);
    userInfo = await response.json();
  } catch {}

  // Fetch user servers from remote api
  let userServers = [];
  try {
    const api = `${process.env.CHAT_BASE_URL}/${process.env.CHAT_GET_USER_SERVERS_ENDPOINT}`;
    const response = await fetch(api, httpGetOptions);
    userServers = await response.json();
  } catch {}

  return {
    props: {
      title: "Chat",
      userInfo: userInfo,
      userServers: userServers,
    },
  };
}

export default function ChatPage({ userInfo, userServers }) {
  const [servers, setServers] = useState([]);
  const [showLoader, setShowLoader] = useState(false);
  const [serverFilter, setServerFilter] = useState(null);
  const [categoryFilter, setCategoryFilter] = useState(null);
  const [selectedServer, setSelectedServer] = useState(null);
  const [serverChannels, setServerChannels] = useState([]);

  useEffect(() => {
    let ignore = false;

    (async function () {
      setShowLoader(true);
      setServers([]);

      let endpoint = "api/getAllServers?name=";
      if (serverFilter) endpoint += `${serverFilter}`;
      if (categoryFilter) endpoint += `&category=${categoryFilter}`;

      try {
        const response = await fetch(endpoint, {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
          },
        });
        const result = await response.json();
        if (!ignore) setServers(result.data);
      } catch (err) {
        console.log("Error fetching servers: ", err);
      }

      setShowLoader(false);
    })();

    return () => {
      ignore = true;
    };
  }, [serverFilter, categoryFilter]);

  const handleServerClick = async function (server) {
    if (!server) {
      setSelectedServer(null);
      setServerChannels([]);
      return;
    }

    try {
      const response = await fetch(
        `api/getServerChannels?serverId=${server.id}&userId=${userInfo.id}`,
        {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
          },
        }
      );

      const result = await response.json();

      setServerChannels(result.data);
      setSelectedServer(server);
      setCategoryFilter(null);
    } catch (err) {
      console.log("Error fetching channels:", err);
    }
  };

  return (
    <>
      <div className={styles.container}>
        <MainNav
          userServers={userServers}
          onServerClick={(server) => {
            handleServerClick(server);
          }}
        />

        {selectedServer ? (
          <ServerSidebar
            serverName={selectedServer.name}
            channels={serverChannels}
          />
        ) : (
          <ExploreSidebar
            selectedCategory={categoryFilter}
            onButtonClick={(item) => {
              setCategoryFilter(item);
            }}
          />
        )}

        <div className={styles.content}>
          {selectedServer ? null : (
            <>
              <ExploreHeader
                onTextChange={(e) => {
                  setServerFilter(e.target.value);
                }}
              />
              <ExploreList servers={servers} showLoader={showLoader} />
            </>
          )}
        </div>
      </div>
    </>
  );
}
