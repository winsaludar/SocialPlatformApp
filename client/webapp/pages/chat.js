import ChatContent from "../src/components/chat/ChatContent";
import ChatHeader from "../src/components/chat/ChatHeader";
import ChatNav from "../src/components/chat/ChatNav";
import ChatSidebar from "../src/components/chat/ChatSidebar";
import styles from "../styles/ChatComponent.module.css";

export async function getStaticProps() {
  return { props: { title: "Chat" } };
}

export default function ChatPage() {
  return (
    <>
      <div className={styles.container}>
        <ChatNav />
        <ChatHeader />
        <ChatSidebar />
        <ChatContent />
      </div>
    </>
  );
}
