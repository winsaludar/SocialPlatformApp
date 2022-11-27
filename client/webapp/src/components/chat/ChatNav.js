import styles from "../../../styles/ChatComponent.module.css";
import Image from "next/image";
export default function ChatNav() {
  return (
    <nav className={styles.mainNav}>
      <ul>
        <li>
          <Image
            src="/images/placeholder/csi-dm.jpg"
            width={50}
            height={50}
            alt="Direct Message"
          />
        </li>

        {[...Array(30)].map((x, i) => {
          return (
            <li key={i}>
              <Image
                src={`/images/placeholder/csi-${(i % 5) + 1}.jpg`}
                width={50}
                height={50}
                alt="Server 1"
              />
            </li>
          );
        })}

        <li>
          <Image
            src="/images/placeholder/csi-explore.jpg"
            width={50}
            height={50}
            alt="Direct Message"
          />
        </li>
      </ul>
    </nav>
  );
}
