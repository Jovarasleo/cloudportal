import { api } from "./api";
const apiPassword = import.meta.env.VITE_API_PASSWORD;

export async function startTorrentService(setData: (data: unknown) => unknown) {
  const toggleTorrentService = api.post({
    url: `https://localhost:7019/StartStopServices/control?action=stop&serviceName=transmission-daemon&sudoPassword=${apiPassword}`,
    callback: setData,
  });

  return toggleTorrentService;
}

export async function statusTorrentService(setData: (data: unknown) => unknown) {
    const getTorrentServiceStatus = api.get({
    url: `https://localhost:7019/ServicesStatus/status?action=stop&serviceName=transmission-daemon&sudoPassword=${apiPassword}`,
    callback: setData,
  });



  return getTorrentServiceStatus;
}



