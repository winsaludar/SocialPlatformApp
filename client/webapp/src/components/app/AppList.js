import { useRouter } from "next/router";

import styles from "../../../styles/AppComponent.module.css";
import Card from "../Card";

export default function AppList({ onItemClick }) {
  const route = useRouter(null);

  return (
    <>
      <div className={styles.grid}>
        <Card
          imageSrc={`/images/placeholder/ali-12.jpg`}
          imageWidth={600}
          imageHeight={400}
          title={`Chat`}
          description="Nullam posuere nibh augue, nec sagittis ex eleifend sed. Curabitur tristique porta consequat. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos."
          onClick={onItemClick}
          styles={styles}
          hasButton={true}
          buttonTitle="Open App"
          buttonOnClick={() => {
            route.push("/chat");
          }}
        />

        {[...Array(10)].map((x, i) => {
          return (
            <Card
              key={i}
              imageSrc={`/images/placeholder/ali-${i + 1}.jpg`}
              imageWidth={600}
              imageHeight={400}
              title={`App Title ${i + 1}`}
              description="Nullam posuere nibh augue, nec sagittis ex eleifend sed. Curabitur tristique porta consequat. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos."
              onClick={onItemClick}
              styles={styles}
              hasButton={true}
              buttonTitle="Open App"
              buttonOnClick={() => {}}
              isDisabled={true}
            />
          );
        })}
      </div>
    </>
  );
}
