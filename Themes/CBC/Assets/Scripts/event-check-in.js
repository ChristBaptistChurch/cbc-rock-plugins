(
    function () {
        this.el = document.createElement('div');

        // Insert elements for chart into body before currently running script
        let allScriptTags = document.getElementsByTagName('script');
        let scriptTag = allScriptTags[allScriptTags.length - 1];

        scriptTag.parentNode.insertBefore(this.el, scriptTag);

        let registrations;
        //9648
        let registrationsPromise = fetch('https://rock.christbaptist.org/api/Registrations?$filter=RegistrationInstanceId eq 13&$expand=Registrants', {
            headers: {
                'Authorization-Token': 'h9WhHVpriCfwhdE9Ytv4sapM'
            }
        });
        let groupMembersPromise = fetch('https://rock.christbaptist.org/api/GroupMembers?$filter=GroupId eq 9648&$expand=Person&$select=Id,Person/NickName,Person/LastName,Person/Id', {
            headers: {
                'Authorization-Token': 'h9WhHVpriCfwhdE9Ytv4sapM'
            }
        });
        
        Promise.all([registrationsPromise, groupMembersPromise]).then((responses) => {
            Promise.all([responses[0].json(), responses[1].json()]).then((json) => {
                let groupMembersById = json[1].reduce((groupMemberMap, thisMember) => {
                    groupMemberMap[thisMember.Person.Id] = thisMember;
                    return groupMemberMap;
                }, {});

                let registrations = json[0];
                console.log(registrations);

                let row = null;
                registrations.forEach((registration, index) => {
                    if (index % 4 == 0) {
                        row = document.createElement('div');
                        row.classList.add("row");
                        this.el.append(row);
                    }

                    row.innerHTML += `
                        <div class="col-md-3">
                            <div class="panel panel-default">
                            <div class="panel-heading">
                                ${registration.LastName}
                            </div>
                    
                            <div class="panel-body">
                                ${registration.Registrants.reduce((text, registrant) => { return text += `<div data-person-id="${registrant.PersonId}"><i class="far fa-square"></i> ${registrant.NickName} ${registrant.LastName}</div>` }, '')}
                            </div>
                            </div>
                        </div>
                    `;
                })
            })
        })
    }
)()


// {% registration where:'RegistrationInstanceId == "13"' sort:'LastName asc' %}
// {% assign items = registrationItems | Sort:'LastName' %}
// {% for item in items %}
//     {% assign newRow = forloop.index0 | Modulo:4 %}

//     {% if newRow == 0 %}
//     <div class="row">
//     {% endif %}

//     <div class="col-md-3">
//     <div class="panel panel-default">
//         <div class="panel-heading">
//             <div class="title">{{ item.LastName }} {%if item.BalanceDue > 0 %}<a class="pull-right" href="https://rock.christbaptist.org/page/405?RegistrationId={{item.Id}}" style="padding: 0px 5px;background-color: red; color: white;">{{ item.BalanceDue | FormatAsCurrency }}</a>{% endif %}</div>
//         </div>

//         <div class="panel-body">


//             {% for person in item.Registrants %}
//                 <div><img src="{{person.Person.PhotoUrl}}&width=100" style="width: 100px;display: block;"><i class="far fa-square"></i> {{ person.NickName }} {{ person.LastName }}</div>
//             {% endfor %}

//         </div>
//     </div>
//     </div>

//     {% if newRow == 3 %}
//     </div>
//     {% endif %}
// {% endfor %}
// {% endregistration %}