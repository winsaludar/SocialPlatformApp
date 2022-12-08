import { sendServerRequest } from "../../src/utils/server.util";

const SAFE_CURRENT_USER_COOKIE = "currentUserHttpOnly";

export default async function handler(req, res) {
  if (req.method !== "GET") {
    return res
      .status(405)
      .json({ errors: ["Unsupported request method"], data: {} });
  }

  // Make sure user cookie (where our auth token reside) exist
  let token;
  try {
    const authCookie = req.cookies[SAFE_CURRENT_USER_COOKIE] ?? null;
    if (!authCookie) throw "Unauthorized user";

    const parsedCookie = JSON.parse(authCookie);
    if (!parsedCookie.value) throw "Unauthorized user";

    token = parsedCookie.value;
  } catch (err) {
    return res.status(404).json({ errors: [err], data: {} });
  }

  // Make sure server id is valid
  if (!req.query || !req.query.serverId)
    return res.status(404).json({ errors: ["Server not found"], data: {} });

  const endpoint = `${process.env.CHAT_GET_SERVER_CHANNELS_ENDPOINT.replace(
    "{{server_id}}",
    req.query.serverId
  )}`;
  const api = `${process.env.CHAT_BASE_URL}/${endpoint}`;

  const options = {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
  };

  return await sendServerRequest(api, options, res);
}
