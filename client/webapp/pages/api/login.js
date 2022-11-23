import { parseServerErrors } from "../../utils/serverUtils";

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

  try {
    const api = `${process.env.AUTH_BASE_URL}/${process.env.AUTH_LOGIN_ENDPOINT}`;
    const options = {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(body),
    };

    const response = await fetch(api, options);
    const result = await response.json();
    if (!response.ok) {
      return res
        .status(response.status)
        .json({ errors: parseServerErrors(result), data: {} });
    }

    return res.status(200).json({ data: result, errors: [] });
  } catch (err) {
    return res
      .status(500)
      .json({ errors: [`Error processing request: ${err}`], data: {} });
  }
}
