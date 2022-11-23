import Head from "next/head";
import { useState } from "react";
import styles from "../../styles/authentication.module.css";
import utilStyles from "../../styles/utils.module.css";
import Link from "next/link";
import AlertBox from "../utils/AlertBox";

export default function Login({ title }) {
  const [formData, setFormData] = useState({});
  const [alertMessages, setAlertMessages] = useState([]);
  const [isLoginSuccessful, setIsLoginSuccessful] = useState(null);

  async function handleSubmit(e) {
    e.preventDefault();

    const payload = JSON.stringify({ ...formData });
    const endpoint = "api/login";
    const options = {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: payload,
    };

    const response = await fetch(endpoint, options);
    const result = await response.json();

    // TODO: DO SOMETHING AFTER LOGIN IS SUCCESSFUL
    if (!response.ok) {
      setIsLoginSuccessful(false);
      setAlertMessages(result.errors);
    } else {
      setIsLoginSuccessful(true);
      setAlertMessages(["Login successful"]);
    }
  }

  return (
    <>
      <Head>
        <title>{title}</title>
      </Head>

      <div className={styles.container}>
        <div className={styles.grid}>
          {alertMessages && alertMessages.length > 0 && (
            <AlertBox
              type={isLoginSuccessful ? "success" : "error"}
              messages={alertMessages}
              onCloseButtonClick={() => setAlertMessages([])}
            />
          )}

          <form className={styles.form} onSubmit={handleSubmit}>
            <div className={styles.field}>
              <label htmlFor="Email" className={styles.label}>
                <svg className={styles.icon}>
                  <use href="#icon-mail"></use>
                </svg>
                <span className={utilStyles.hidden}>Email</span>
              </label>
              <input
                type="email"
                className={styles.input}
                id="Email"
                name="Email"
                placeholder="Email"
                autoComplete="on"
                required
                onChange={(e) =>
                  setFormData({ ...formData, email: e.target.value })
                }
              />
            </div>

            <div className={styles.field}>
              <label htmlFor="password" className={styles.label}>
                <svg className={styles.icon}>
                  <use href="#icon-lock"></use>
                </svg>
                <span className={utilStyles.hidden}>Password</span>
              </label>
              <input
                type="password"
                className={styles.input}
                id="password"
                name="password"
                placeholder="Password"
                autoComplete="off"
                required
                onChange={(e) =>
                  setFormData({ ...formData, password: e.target.value })
                }
              />
            </div>

            <div className={styles.field}>
              <button type="submit" className={styles.submit}>
                Sign In
              </button>
            </div>
          </form>

          <p className={styles.text}>
            Not a member? <Link href="/register">Sign up now</Link>{" "}
            <svg className={styles.icon}>
              <use href="#icon-arrow-right"></use>
            </svg>
          </p>
        </div>

        <svg xmlns="http://www.w3.org/2000/svg" className={styles.icons}>
          <symbol id="icon-mail" viewBox="0 0 20 20">
            <path d="M17.051,3.302H2.949c-0.866,0-1.567,0.702-1.567,1.567v10.184c0,0.865,0.701,1.568,1.567,1.568h14.102c0.865,0,1.566-0.703,1.566-1.568V4.869C18.617,4.003,17.916,3.302,17.051,3.302z M17.834,15.053c0,0.434-0.35,0.783-0.783,0.783H2.949c-0.433,0-0.784-0.35-0.784-0.783V4.869c0-0.433,0.351-0.784,0.784-0.784h14.102c0.434,0,0.783,0.351,0.783,0.784V15.053zM15.877,5.362L10,9.179L4.123,5.362C3.941,5.245,3.699,5.296,3.581,5.477C3.463,5.659,3.515,5.901,3.696,6.019L9.61,9.86C9.732,9.939,9.879,9.935,10,9.874c0.121,0.062,0.268,0.065,0.39-0.014l5.915-3.841c0.18-0.118,0.232-0.36,0.115-0.542C16.301,5.296,16.059,5.245,15.877,5.362z"></path>
          </symbol>
          <symbol id="icon-lock" viewBox="0 0 1792 1792">
            <path d="M640 768h512V576q0-106-75-181t-181-75-181 75-75 181v192zm832 96v576q0 40-28 68t-68 28H416q-40 0-68-28t-28-68V864q0-40 28-68t68-28h32V576q0-184 132-316t316-132 316 132 132 316v192h32q40 0 68 28t28 68z" />
          </symbol>
          <symbol id="icon-arrow-right" viewBox="0 0 1792 1792">
            <path d="M1600 960q0 54-37 91l-651 651q-39 37-91 37-51 0-90-37l-75-75q-38-38-38-91t38-91l293-293H245q-52 0-84.5-37.5T128 1024V896q0-53 32.5-90.5T245 768h704L656 474q-38-36-38-90t38-90l75-75q38-38 90-38 53 0 91 38l651 651q37 35 37 90z" />
          </symbol>
        </svg>
      </div>
    </>
  );
}
