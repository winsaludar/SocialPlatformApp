import https from "https";

export default async function handler(req, res) {
  if (req.method !== "POST")
    return res
      .status(405)
      .json({ error: "Unsupported request method", data: {} });

  const body = req.body;
  if (!body.email || !body.password) {
    return res
      .status(400)
      .json({ error: "Required fields cannot be empty", data: {} });
  }

  try {
    const api = `${process.env.AUTH_BASE_URL}/${process.env.AUTH_LOGIN_ENDPOINT}`;
    const options = {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: body,
      agent: new https.Agent({ rejectUnauthorized: false }),
    };

    console.log(api);

    const response = await fetch(api, options);
    console.log("Response: ", response);

    const result = await response.json();
    console.log("Result ", result);

    return res.status(200).json({ data: result, error: null });
  } catch (err) {
    console.log("Err ", err);

    return res
      .status(500)
      .json({ error: `Error processing request: ${err}`, data: {} });
  }
}
