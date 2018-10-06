namespace FSharpMicroservice.Controllers

open Microsoft.AspNetCore.Mvc
open Notifier

[<CLIMutable>]
type NotificationRequest = {
    UserId : string
    Message : string
}

[<Route("api/[controller]")>]
[<ApiController>]
type NotificationController () =
    inherit ControllerBase()

    [<HttpGet>]
    member __.Get() =
        let values = [|"it"; "is"; "working"|]
        ActionResult<string[]>(values)        

    [<HttpPost>]
    member __.Post([<FromBody>] request : NotificationRequest) =
        NotifyUser request.UserId request.Message
