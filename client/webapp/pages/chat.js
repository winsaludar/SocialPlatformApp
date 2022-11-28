import ExploreList from "../src/components/chat/ExploreList";
import ExploreHeader from "../src/components/chat/ExploreHeader";
import MainNav from "../src/components/chat/MainNav";
import Sidebar from "../src/components/chat/Sidebar";
import styles from "../styles/ChatComponent.module.css";

export async function getStaticProps() {
  return { props: { title: "Chat" } };
}

export default function ChatPage() {
  return (
    <>
      <div className={styles.container}>
        <MainNav />
        <Sidebar />

        <div className={styles.content}>
          <ExploreHeader />
          <ExploreList />
        </div>
      </div>
    </>
  );
}
