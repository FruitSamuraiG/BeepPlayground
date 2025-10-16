# :heart: ![](https://placehold.co/900x60/transparent/pink/?text=BeepPlayground+(CSHARP)&font=roboto)
- This program uses Windows Console funtion to play Beep-sound with different frequencies and durations of beep
- I used beeps to play with some things I wanted to learn. So here's the list of features:
  - Music intro with prepared beep's data and ASCII-images
  - TCP Server/Client exchanging beep's data (both work asynchronous in one process)
  - HTTP-request to Hugging Face API for user's choosen song (result is parsed by regex to extract beep's data)
  - Inserting and getting beepsongs into/from PostgreSQL Database (from the Hugging Face API)
- Builded program with file examples and source code are here for you to try things and learn something, don't forget to change API key in the source code!
# :smiley: ![](https://placehold.co/900x60/transparent/pink/?text=My+Contacts&font=roboto)
  - Telegram: @toshiforlife
  - E-mail: ilya-druchenko@mail.ru
# 💻 ![](https://placehold.co/900x60/transparent/pink/?text=Documentation&font=roboto) 💻
## Test.cs
- This file contains Main function of program, just some sort of interface for users.
## DatabaseCommunication.cs
- This file contains PostgreSQL functions of program
<table>
    <tr>
        <th>Function</th>
        <th>Description</th>
    </tr>
    <tr>
        <td>void GetSongsFromDatabase(string connectionConfig, ref string container)</td>
        <td>Returns a list of song names from database as single string into container</td>
    </tr>
    <tr>
        <td>string GetBeepsDataFromDatabase(string connectionConfig, string songName)</td>
        <td>Returns beeps data as single string if song is found in database</td>
    </tr>
    <tr>
        <td>void SaveIntoDatabase(string connectionConfig, string songName, string beepsData)</td>
        <td>Loads song with beepdata into database</td>
    </tr>
</table>

## Intro.cs
- This file contains general functionality of beep interaction
<table>
    <tr>
        <th>Function</th>
        <th>Description</th>
    </tr>
    <tr>
    <th>async Task PlayBeepFromHTTP()</th>
        <th>
        Asks user to write song name and then makes an to Hugging Face API to get beeps data.
        Play Beeps, after that calls SaveIntoDatabase to save song with beepdata into database.
        </th>
    </tr>
    <tr>
        <th>async Task PlayIntroTCP()</th>
        <th>
        Starts TCP-Server and TCP-Client asynchronously. Client recieves beeps data from file and sends it to server.
        Server recievies data from client, processes it and then plays beeps.
        </th>
    </tr>
    <tr>
        <th>async Task <int> PlayIntro()</th>
        <th>
          Reads beeps data from file, reads ASCII-images from files, then asynchronously plays beep and prints images into console. Returns 0 if everything worked correctly, else returns -1.
        </th>
    </tr>
</table>

## TCPServerAndClient.cs
- This file contains Server and Client functions 👀
<table>
  <tr>
    <th>Function</th>
    <th>Description</th>
  </tr>
  <tr>
    <th>async Task<string> Server(CancellationTokenSource cts)</th>
    <th>Starts TCP-Server, after getting and proceeding data from Client returns result</th>
  </tr>
  <tr>
    <th>async Task Client(string pathToFile, IPEndPoint serverAddress, CancellationTokenSource cts)</th>
    <th>Starts TCP-Client, Client reads beeps data from file and sends it to the Server</th>
  </tr>
</table>

# :pizza: ![](https://placehold.co/900x60/transparent/pink/?text=Pizza!&font=roboto)
![](https://tenor.com/view/limbus-company-rodion-limbus-company-gif-5051786804155307089.gif)
