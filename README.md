# Totp
Initialing 'Totp' using dotnet 
 - In this codes , there are 2 controller that in 'register' controller, we create 
 the Qr code from the secret key and save the secret key and userName in the dictionary,
 and return the Qr code image as base64
 - In 'login' controller , after user uses the , google athenticator or some app like it to scan the code
 enter the code in this controller and verify it and successfully login