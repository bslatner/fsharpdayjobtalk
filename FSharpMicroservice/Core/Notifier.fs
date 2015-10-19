module Notifier

open FSharp.Data
open Microsoft.FSharp.Data.TypeProviders
open ApiConfig

type ServiceDB = SqlDataConnection<"Data Source=localhost; Initial Catalog=Test; Integrated Security=True;">

type NotificationDestination =
    | SmsNumber of string
    | EmailAddress of string

let SendSms destination (message : string) =
    let client = new Twilio.TwilioRestClient(TwilioAccountSid, TwilioAccountKey)
    client.SendMessage("+19802482928", destination, message)

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
    client.SendEmail(request)

let SendNotification (destination : NotificationDestination) message =
    match destination with
    | SmsNumber number -> SendSms number message |> ignore
    | EmailAddress email -> SendEmail email message |> ignore

let NotifyUser userId message =

    let getSmsDestination (d : ServiceDB.ServiceTypes.NotificationSms) =
        SmsNumber d.PhoneNumber

    let getEmailDestination (d : ServiceDB.ServiceTypes.NotificationEmail) =
        EmailAddress d.Email

    use db = ServiceDB.GetDataContext()
    let dests = seq {
        yield! query {
            for sms in db.NotificationSms do
            where (sms.UserId = userId)
            select sms
        } |> Seq.map getSmsDestination

        yield! query {
            for email in db.NotificationEmail do
            where (email.UserId = userId)
            select email
        } |> Seq.map getEmailDestination
    }

    dests |> Seq.iter (fun d -> SendNotification d message)
