import { sendServerRequest } from "../../src/utils/server.util";

export default async function handler(req, res) {
  if (req.method !== "POST")
    return res
      .status(405)
      .json({ errors: ["Unsupported request method"], data: {} });

  const body = req.body;
  if (
    !body.firstName ||
    !body.lastName ||
    !body.email ||
    !body.retypeEmail ||
    !body.password ||
    !body.retypePassword
  ) {
    return res
      .status(400)
      .json({ errors: ["All fields are required"], data: {} });
  }

  const errors = [];
  if (body.email !== body.retypeEmail)
    errors.push("Email and Re-type Email does not match");
  if (body.password !== body.retypePassword)
    errors.push("Password and Re-type Password does not match");
  if (errors.length > 0) {
    return res.status(400).json({ errors: errors, data: {} });
  }

  const api = `${process.env.AUTH_BASE_URL}/${process.env.AUTH_REGISTER_ENDPOINT}`;
  const options = {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(body),
  };

  return await sendServerRequest(api, options, res);
}
