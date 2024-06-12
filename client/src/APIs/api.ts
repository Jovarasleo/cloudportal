interface FetchOptions {
  url: string;
  method?: string;
  headers?: Record<string, string>;
  body?: any;
  callback?: (response: Response) => void;
}

export const api = {
  fetch: (options: FetchOptions) => {
    const { url, method = "GET", headers = {}, body, callback } = options;

    fetch(url, {
      method,
      headers,
      body: method === "GET" ? undefined : JSON.stringify(body),
    })
      .then((response) => {
        if (callback) {
          callback(response);
        } else {
          return response.json();
        }
      })
      .then((data) => {
        if (!callback) {
          console.log(data);
        }
      })
      .catch((error) => {
        console.error("Error:", error);
      });
  },

  post: (options: FetchOptions) => {
    options.method = "POST";
    return api.fetch(options);
  },

  get: (options: FetchOptions) => {
    options.method = "GET";
    return api.fetch(options);
  },
};
