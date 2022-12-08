import { parseServerErrors } from "../../src/utils/server.util";

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
  if (!req.query || !req.query.serverId) {
    return res.status(404).json({ errors: ["Server not found"], data: {} });
  }

  // Make sure user id is included
  if (!req.query || !req.query.userId) {
    return res.status(400).json({ errors: ["User id is invalid"], data: {} });
  }

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

  // Call remote api
  try {
    const response = await fetch(api, options);
    const result = await response.json();
    if (!response.ok) {
      return res
        .status(response.status)
        .json({ errors: parseServerErrors(result), data: {} });
    }

    // Return only channels that are either public or user is a member
    let channels = [];
    if (Array.isArray(result)) {
      channels = result.filter(
        (x) => x.isPublic || x.members.includes(req.query.userId)
      );
    }

    return res.status(200).json({ data: channels, errors: [] });
  } catch (err) {
    return res.status(500).json({
      errors: [
        "Unable to process request at this moment, please try again later",
      ],
      data: {},
    });
  }
}
