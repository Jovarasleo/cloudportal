<script lang="ts">
  import { startTorrentService, statusTorrentService } from './APIs/services';

    let loading = true;
    let error = null;
    let postData: unknown= []

  async function handleServiceToggle() {
    loading = true;
    error = null;

    try {
      await startTorrentService((data)=>postData=data)
    } catch (err) {
      error = "Error fetching data: ";
    } finally {
      loading = false;
    }
  }

  async function handleServiceStatus() {
    loading = true;
    error = null;

    try {
      await statusTorrentService((data)=>postData=data)
    } catch (err) {
      error = "Error fetching data: ";
    } finally {
      loading = false;
    }
  }
</script>

<main>
  <button on:click={handleServiceToggle}>Start torrent</button>
  <button on:click={handleServiceStatus}>Get torrent status</button>
</main>

<style>
</style>
