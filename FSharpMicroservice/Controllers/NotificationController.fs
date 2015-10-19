namespace FSharpMicroservice.Controllers
open System
open System.Collections.Generic
open System.Linq
open System.Net.Http
open System.Web.Http
open FSharpMicroservice.Models
open Notifier

/// Retrieves values.
type NotificationController() =
    inherit ApiController()

    /// Post notification to user
    member x.Post(request : NotificationRequest) = 
        NotifyUser request.UserId request.Message