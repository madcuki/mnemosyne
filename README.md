I built this project to create a secure, localized system for storing and retrieving credentials without relying on third-party services. The goal was to ensure complete control over sensitive data while maintaining strong encryption and customizable password generation.

The system features AES-GCM-SIV encryption to protect stored credentials, chosen for its resistance to common cryptographic attacks. Additionally, it includes a highly customizable password generator, allowing users to tailor password complexity to their needs.

Finding a library that implemented AES-GCM-SIV proved difficult, as it is a less commonly supported encryption standard. I ultimately integrated the Bouncy Castle library to implement this encryption effectively.

Key Technologies: C#, JSON, Bouncy Castle

Future Considerations:
This project was originally built for Windows, which I no longer use. If I were to rebuild this project today, I would opt for a cross-platform language like Python to improve accessibility across different environments.
