module Notifier

open FSharp.Data.Sql
open Twilio.Clients
open Twilio.Rest.Api.V2010.Account
open Twilio.Types
open ApiConfig


[<Literal>]
let ConnectionString = "Data Source=localhost; Initial Catalog=Test; Integrated Security=True;"

type ServiceDB = SqlDataProvider<Common.DatabaseProviderTypes.MSSQLSERVER, ConnectionString>

type NotificationDestination =
    | SmsNumber of string
    | EmailAddress of string

let SendSms destination (message : string) =
    let client = TwilioRestClient(TwilioAccountSid, TwilioAccountKey) :> ITwilioRestClient
    let opts = CreateMessageOptions(PhoneNumber(destination))
    opts.From <- PhoneNumber(OutboundPhoneNumber)
    opts.Body <- message
    MessageResource.CreateAsync(opts, client) |> Async.AwaitTask

let SendEmail destination message =
    let client = new Amazon.SimpleEmail.AmazonSimpleEmailServiceClient(AwsAccessKey, AwsSecretKey, Amazon.RegionEndpoint.USEast1)
    let request = new Amazon.SimpleEmail.Model.SendEmailRequest()
    request.Source <- "bryan@slatner.com"
    request.Destination <- new Amazon.SimpleEmail.Model.Destination()
    request.Destination.ToAddresses.Add(destination)
    request.Message <- new Amazon.SimpleEmail.Model.Message()
    request.Message.Subject <- new Amazon.SimpleEmail.Model.Content("Notification message")
    let bodyContent = new Amazon.SimpleEmail.Model.Content(message)
    request.Message.Body <- new Amazon.SimpleEmail.Model.Body(bodyContent)
    client.SendEmailAsync(request) |> Async.AwaitTask

let SendNotification (destination : NotificationDestination) message =
    match destination with
    | SmsNumber number -> SendSms number message |> Async.Ignore
    | EmailAddress email -> SendEmail email message |> Async.Ignore

let NotifyUser userId message =

    let getSmsDestination (d : ServiceDB.dataContext.``dbo.NotificationSmsEntity``) =
        SmsNumber d.PhoneNumber

    let getEmailDestination (d : ServiceDB.dataContext.``dbo.NotificationEmailEntity``) =
        EmailAddress d.Email

    let db = ServiceDB.GetDataContext()
    let dests = seq {
        yield! query {
            for sms in db.Dbo.NotificationSms do
            where (sms.UserId = userId)
            select sms
        } |> Seq.map getSmsDestination

        yield! query {
            for email in db.Dbo.NotificationEmail do
            where (email.UserId = userId)
            select email
        } |> Seq.map getEmailDestination
    }

    dests 
    |> Seq.map (fun d -> SendNotification d message)
    |> Async.Parallel
    |> Async.RunSynchronously
