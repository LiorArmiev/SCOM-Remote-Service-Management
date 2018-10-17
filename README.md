# SCOM-Remote-Service-Management

# Project Title

This project is creating in order to help users of SCOM to manage remote services from one place without the need of monitoring the services themselves.
the idia is to us SCOM's ability to run remote tasks in order to control services on remote computers.

## Getting Started

If you know how SCOM SDK works this project will be very simple to undersand, its craetes a simple table and runs a task on the server input.
the task returns a list of services and formats it in the table.
later you have some command buttons like stop and start that too run remote task in SCOM with the stop & start commands.

thats it.

one more point is an update that i made that allows you to change the user runnigng the task.
in most of the cases its the user that is loged on, but in cases of servers on remote domains you need a defrent user and now you can change it.

every task that is running in the web is saving an audit about the activity in the task result table of SCOM so you can always see who did waht.

Have Fun

### Prerequisites

in order to install this website you need to have .Net 4.6 
