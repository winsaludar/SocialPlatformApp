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
