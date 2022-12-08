import styles from "../../../styles/ChatComponent.module.css";

export default function ServerSidebar({ serverName, channels, onButtonClick }) {
  console.log("Sidebar:", channels);

  return (
    <aside className={`${styles.sidebar} ${styles.smallHeader}`}>
      <h2>{serverName}</h2>

      <div>
        {Array.isArray(channels) &&
          channels.map((channel) => {
            return (
              <button
                key={channel.id}
                type="button"
                onClick={() => (onButtonClick ? onButtonClick(channel) : null)}
              >
                {channel.name}
              </button>
            );
          })}
      </div>
    </aside>
  );
}
