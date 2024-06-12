import { api } from "./api";

export async function startTorrentService(setData: (data: unknown) => unknown) {
  const toggleTorrentService = api.get({
    url: "http://localhost:5126/StartStopServices/control?action=start&serviceName=transmission",
    callback: setData,
  });

  return toggleTorrentService;
}
