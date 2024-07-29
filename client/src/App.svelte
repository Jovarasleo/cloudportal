<script lang="ts">
  import { startTorrentService } from './APIs/services';

    let loading = true;
    let error = null;
    let postData: unknown= []

    async function handleButtonClick() {
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
</script>

<main>
  <button on:click={handleButtonClick}>Hit endpoint</button>
</main>

<style>
</style>
