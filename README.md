# ProPresenter Vmix Bridge

This project consists of a Windows Service that acts like a bridge between ProPresenter and Vmix.
It works by using ProPresenter's WebSocket Stage Display API to receive slide changes and injects the current slide's text into a VMix Title/Xaml Input.
