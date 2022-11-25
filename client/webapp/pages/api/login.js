import { sendServerRequest } from "../../src/utils/serverUtils";

export default async function handler(req, res) {
  if (req.method !== "POST")
    return res
      .status(405)
      .json({ errors: ["Unsupported request method"], data: {} });

  const body = req.body;
  if (!body.email || !body.password) {
    return res
      .status(400)
      .json({ errors: ["Required fields cannot be empty"], data: {} });
  }

  const api = `${process.env.AUTH_BASE_URL}/${process.env.AUTH_LOGIN_ENDPOINT}`;
  const options = {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(body),
  };

  return await sendServerRequest(api, options, res);
}
