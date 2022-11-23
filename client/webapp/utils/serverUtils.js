export function parseServerErrors(result) {
  const errors = [];

  if (result.hasOwnProperty("errors") && typeof result.errors === "object") {
    for (const [key, value] of Object.entries(result.errors)) {
      if (Array.isArray(value)) value.map((x) => errors.push(x));
      if (typeof value === "string") errors.push(value);
    }
  }

  if (result.hasOwnProperty("error") && typeof result.error === "string") {
    errors.push(result.error);
  }

  return errors;
}

export async function sendServerRequest(api, fetchOptions, res) {
  try {
    const response = await fetch(api, fetchOptions);
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
