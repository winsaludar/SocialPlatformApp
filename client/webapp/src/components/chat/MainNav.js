import Image from "next/image";

import styles from "../../../styles/ChatComponent.module.css";

export default function MainNav({ userServers, onServerClick }) {
  return (
    <nav className={styles.mainNav}>
      <ul>
        <li>
          <Image
            src="/images/placeholder/csi-dm.jpg"
            width={50}
            height={50}
            alt="Messages"
          />
        </li>

        {Array.isArray(userServers) &&
          userServers.map((server) => {
            return (
              <li key={server.id} onClick={() => onServerClick(server)}>
                <Image
                  src={server.thumbnail}
                  width={50}
                  height={50}
                  alt={server.name}
                />
              </li>
            );
          })}

        <li onClick={() => onServerClick(null)}>
          <Image
            src="/images/placeholder/csi-explore.jpg"
            width={50}
            height={50}
            alt="Explore"
          />
        </li>
      </ul>
    </nav>
  );
}
