import Link from "next/link";
import { useState } from "react";

import styles from "../../../styles/authentication.module.css";
import utilStyles from "../../../styles/utils.module.css";
import AlertBox from "../AlertBox";

export default function Register() {
  const [formData, setFormData] = useState({});
  const [alertMessages, setAlertMessages] = useState([]);
  const [isRegisterSuccessful, setIsRegisterSuccessful] = useState(null);

  async function handleSubmit(e) {
    e.preventDefault();

    const payload = JSON.stringify({ ...formData });
    const endpoint = "api/register";
    const options = {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: payload,
    };

    const response = await fetch(endpoint, options);
    const result = await response.json();

    if (!response.ok) {
      setAlertMessages(result.errors);
      setIsRegisterSuccessful(false);
    } else {
      setAlertMessages([
        "Registration successful. You may now use your account to login",
      ]);
      setIsRegisterSuccessful(true);
    }
  }

  return (
    <>
      <div className={styles.container}>
        <div className={styles.grid}>
          {alertMessages && alertMessages.length > 0 && (
            <AlertBox
              type={isRegisterSuccessful ? "success" : "error"}
              messages={alertMessages}
              onCloseButtonClick={() => setAlertMessages([])}
            />
          )}

          <form className={styles.form} onSubmit={handleSubmit}>
            {/* First Name */}
            <div className={styles.field}>
              <label htmlFor="firstName" className={styles.label}>
                <svg className={styles.icon}>
                  <use href="#icon-user"></use>
                </svg>
                <span className={utilStyles.hidden}>First Name</span>
              </label>
              <input
                type="text"
                className={styles.input}
                id="firstName"
                name="firstName"
                placeholder="First Name"
                autoComplete="on"
                required
                onChange={(e) =>
                  setFormData({ ...formData, firstName: e.target.value })
                }
              />
            </div>

            {/* Last Name */}
            <div className={styles.field}>
              <label htmlFor="lastName" className={styles.label}>
                <svg className={styles.icon}>
                  <use href="#icon-user"></use>
                </svg>
                <span className={utilStyles.hidden}>Last Name</span>
              </label>
              <input
                type="text"
                className={styles.input}
                id="lastName"
                name="lastName"
                placeholder="Last Name"
                autoComplete="on"
                required
                onChange={(e) =>
                  setFormData({ ...formData, lastName: e.target.value })
                }
              />
            </div>

            {/* Email */}
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

            {/* Re-type Email */}
            <div className={styles.field}>
              <label htmlFor="retypeEmail" className={styles.label}>
                <svg className={styles.icon}>
                  <use href="#icon-mail"></use>
                </svg>
                <span className={utilStyles.hidden}>Re-type Email</span>
              </label>
              <input
                type="email"
                className={styles.input}
                id="retypeEmail"
                name="retypeEmail"
                placeholder="Re-type Email"
                autoComplete="on"
                required
                onChange={(e) =>
                  setFormData({ ...formData, retypeEmail: e.target.value })
                }
              />
            </div>

            {/* Password */}
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

            {/* Re-type Password */}
            <div className={styles.field}>
              <label htmlFor="retypePassword" className={styles.label}>
                <svg className={styles.icon}>
                  <use href="#icon-lock"></use>
                </svg>
                <span className={utilStyles.hidden}>Re-type Password</span>
              </label>
              <input
                type="password"
                className={styles.input}
                id="retypePassword"
                name="retypePassword"
                placeholder="Re-type Password"
                autoComplete="off"
                required
                onChange={(e) =>
                  setFormData({ ...formData, retypePassword: e.target.value })
                }
              />
            </div>

            <div className={styles.field}>
              <button type="submit" className={styles.submit}>
                Sign up
              </button>
            </div>
          </form>

          <p className={styles.text}>
            Already have an account? <Link href="/">Sign in here</Link>{" "}
            <svg className={styles.icon}>
              <use href="#icon-arrow-right"></use>
            </svg>
          </p>
        </div>

        <svg xmlns="http://www.w3.org/2000/svg" className={styles.icons}>
          <symbol id="icon-user" viewBox="0 0 1792 1792">
            <path d="M1600 1405q0 120-73 189.5t-194 69.5H459q-121 0-194-69.5T192 1405q0-53 3.5-103.5t14-109T236 1084t43-97.5 62-81 85.5-53.5T538 832q9 0 42 21.5t74.5 48 108 48T896 971t133.5-21.5 108-48 74.5-48 42-21.5q61 0 111.5 20t85.5 53.5 62 81 43 97.5 26.5 108.5 14 109 3.5 103.5zm-320-893q0 159-112.5 271.5T896 896 624.5 783.5 512 512t112.5-271.5T896 128t271.5 112.5T1280 512z" />
          </symbol>
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
