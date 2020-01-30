# ProPresenter Vmix Bridge

This project consists of a Windows Service that acts like a bridge between ProPresenter and Vmix.
It works by using ProPresenter's WebSocket Stage Display API to receive slide changes and injects the current slide's text into a VMix Title/Xaml Input.

THIS VERSION CURRENTLY ONLY WORKS WITH PROPRESENTER 5 AND 6. IT IS NOT COMPATIBLE WITH PROPRESENTER 7. 

Here are some basic use and installation instructions:

1. In ProPresenter 5 or 6, go to Preferences -> Network. Check the Enable Network box and give it a meaningful name.

2. Check the Enable Stage Display box and enter a password.

3. On the vMix computer, go to the GitHub page. Select Releases and download the newest version of ProPresenterVmixBridgeSetup.msi.

4. Install Apple Bonjour on the vMix computer (the bridge program will crash without this!)

5. Install the ProPresenterVmixBridgeSetup program.

6. Run the ProPresenterVmixBridge configuration app as an administrator.

7. In the ProPresenter section, the ProPresenter should already be listed (found by Boujour).

8. Enter the stage display password.

9. In the vMix section, the IP should be 127.0.0.1 and the port should be 8088. These are the default values.

10. For the input number, this is the vMix input number. For example, if your lower third title will be input five, enter 5 here.

11. Click apply and close the configuration app.

12. In vMix add a title input for your lower third and configure it the way you want. Note, you do not need to add a data source to this title.

13. Make sure the title input number matches the one you set up in ProPresenterVmixBridge config.

14. Click a slide in ProPresenter. The lower third in vMix should now contain the slide text.

Note that ProPresenterVmixBridge runs as a service, so the config app does not need to remain running.

