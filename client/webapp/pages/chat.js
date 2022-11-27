import ChatContent from "../src/components/chat/ChatContent";
import ChatHeader from "../src/components/chat/ChatHeader";
import ChatNav from "../src/components/chat/ChatNav";
import ChatSidebar from "../src/components/chat/ChatSidebar";

export async function getStaticProps() {
  return { props: { title: "Chat" } };
}

export default function ChatPage() {
  return (
    <>
      <nav>
        <ChatNav />
      </nav>

      <div>
        <header>
          <ChatHeader />
        </header>

        <aside>
          <ChatSidebar />
        </aside>

        <section>
          <ChatContent />
        </section>
      </div>
    </>
  );
}
