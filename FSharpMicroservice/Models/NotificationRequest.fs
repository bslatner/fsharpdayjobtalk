namespace FSharpMicroservice.Models

open Newtonsoft.Json

[<CLIMutable>]
type NotificationRequest = {
    UserId : string
    Message : string
}
